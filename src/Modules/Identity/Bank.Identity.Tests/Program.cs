using Microsoft.AspNetCore.Builder;

// Use Bank.Host's Program class for testing
var builder = WebApplication.CreateBuilder(args);

// This is required for the test factory to use the correct Program class
// The actual configuration comes from Bank.Host
