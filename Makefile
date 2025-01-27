build:
	dotnet build

run-api:
	dotnet run --project src/notification/api/Notification.Api.csproj

run-test:
	dotnet test

run-notification-test:
	dotnet test --filter "UnitTest=NotificationTest"

run-local-cache-provider-test:
	dotnet test --filter "UnitTest=LocalMemoryCache"

run-rate-limit-rules-test:
	dotnet test --filter "UnitTest=RateLimitRuleTest"

run-expiration-parser-test:
	dotnet test --filter "UnitTest=ExpirationParserInput"