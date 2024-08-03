build:
	dotnet build

run-test:
	dotnet test

run-notification-test:
	dotnet test --filter "UnitTest=NotificationTest"

run-local-cache-provider-test:
	dotnet test --filter "UnitTest=LocalMemoryCache"