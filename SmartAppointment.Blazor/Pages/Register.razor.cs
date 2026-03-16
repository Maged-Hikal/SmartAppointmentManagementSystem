using System.Net.Http.Json;
using System.Text.Json;

namespace SmartAppointment.Blazor.Pages
{
    public partial class Register
    {
        private List<string> errors = new();
        private RegisterModel _model = new();
        private bool showPassword = false;


        private async Task HandleRegistration()
        {
            errors.Clear();
            try
            {
                var response = await Http.PostAsJsonAsync("api/me/register", _model);

                if (response.IsSuccessStatusCode)
                {
                    Nav.NavigateTo("/login");
                }
                else
                {
                    // Read the raw string first to see if it's empty
                    var rawError = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(rawError))
                    {
                        // Try to parse the JSON list of strings
                        var errorData = JsonSerializer.Deserialize<List<string>>(rawError,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (errorData != null) errors = errorData;
                    }
                    else
                    {
                        errors.Add("An error occurred but no details were provided.");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add("Connection error: " + ex.Message);
            }
        }
    }
}
