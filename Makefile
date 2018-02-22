all: clean run

build:
	dotnet build -c Release tests
run:
	dotnet run --project tests

clean:
	rm -rf tests/obj
	rm -rf tests/bin
	find tests -name "*.tmp" -print | xargs rm -f
