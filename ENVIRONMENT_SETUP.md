# Environment Variables Configuration

Set these environment variables for different deployment environments:

## Development (Default values)
```bash
API_BASE_URL=https://localhost:7216
BLAZOR_URL=https://localhost:7161
```

## Production
```bash
API_BASE_URL=https://api.yourdomain.com
BLAZOR_URL=https://yourdomain.com
```

## Staging
```bash
API_BASE_URL=https://staging-api.yourdomain.com
BLAZOR_URL=https://staging.yourdomain.com
```

## Docker/Container Deployment
```bash
docker run -e API_BASE_URL=https://api.yourdomain.com -e BLAZOR_URL=https://yourdomain.com your-app
```

## Azure App Service
Set in Configuration > Application settings:
- API_BASE_URL: https://api.yourdomain.com
- BLAZOR_URL: https://yourdomain.com

## IIS Web.config
```xml
<environmentVariables>
    <add name="API_BASE_URL" value="https://api.yourdomain.com" />
    <add name="BLAZOR_URL" value="https://yourdomain.com" />
</environmentVariables>
```
