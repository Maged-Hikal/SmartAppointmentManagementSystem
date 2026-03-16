using Microsoft.AspNetCore.Components;
using SmartAppointment.Blazor.Models;
using System.Net.Http.Json;

namespace SmartAppointment.Blazor.Pages
{
    public partial class Appointments
    {
        [Inject] private HttpClient? Http { get; set; }

        private List<AppointmentModel>? appointments;
        private HashSet<string> _userPermissions = new(StringComparer.OrdinalIgnoreCase);

        protected override async Task OnInitializedAsync()
        {
            var appointmentsTask = Api.GetAllAsync();
            var permsTask = Http!.GetFromJsonAsync<List<string>>("api/me/permissions");

            appointments = await appointmentsTask;
            var perms = await permsTask ?? new List<string>();
            _userPermissions = perms.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private async Task Approve(Guid id)
        {
            await Api.ApproveAsync(id);
            appointments = await Api.GetAllAsync();
        }
        private async Task Reject(Guid id)
        {
            await Api.RejectAsync(id);
            appointments = await Api.GetAllAsync();
        }
        private async Task CancelAsync(Guid id)
        {
            await Api.CancelAsync(id);
            appointments = await Api.GetAllAsync();
        }
        private bool showModal = false;
        private Guid selectedId;
        private string? targetCustomerName;
        private DateTime rescheduleDate = DateTime.Today;

        private List<TimeSpan> TimeSlots = new()
        {
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 0, 0),
            new TimeSpan(16, 0, 0),
            new TimeSpan(17, 0, 0)
        };

        async Task ConfirmReschedule(TimeSpan slot)
        {
            DateTime combined = rescheduleDate.Date.Add(slot);

            DateTime finalDateTime = DateTime.SpecifyKind(combined, DateTimeKind.Unspecified);

            await Api.RescheduleAsync(selectedId, finalDateTime);

            showModal = false;
            appointments = await Api.GetAllAsync();
        }

        bool IsSelected(TimeSpan slot) => rescheduleDate.TimeOfDay == slot;
        void CloseModal() => showModal = false;
        void PrepareReschedule(Guid id, string? name, DateTime currentDate)
        {
            selectedId = id;
            targetCustomerName = name;
            rescheduleDate = currentDate.Date;
            showModal = true;
        }

        private bool HasPerm(string? name) => _userPermissions.Contains(name);

        private static bool IsPending(string? status) =>
            string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase);

        private bool CanApprove(string? status) =>
            IsPending(status) && HasPerm("Appointments.Approve");

        private static bool IsApprovedOrRescheduled(string? status) =>
            string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Rescheduled", StringComparison.OrdinalIgnoreCase);

        private bool CanReschedule(string? status) =>
            IsApprovedOrRescheduled(status) && HasPerm("Appointments.Reschedule");

        private static bool IsCancelableStatus(string? status) =>
            string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Rescheduled", StringComparison.OrdinalIgnoreCase);

        private bool CanCancel(string? status) =>
            IsCancelableStatus(status) && HasPerm("Appointments.Cancel");

        private static bool IsDeletableStatus(string? status) =>
            string.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase);

        private bool CanDelete(string? status) =>
            IsDeletableStatus(status) &&
            (HasPerm("Appointments.Reject") || HasPerm("Appointments.Reject")); // tolerate seed typo
    }
}
