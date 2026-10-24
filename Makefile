# Optional GNU Make shortcuts for the Windows .NET 8 build.
DOTNET ?= dotnet
CONFIG ?= Release

all:
	$(DOTNET) build SPICA.sln -c $(CONFIG)

install:
	$(DOTNET) publish SPICA.WinForms -c $(CONFIG) -r win-x64 --self-contained true -o publish

run:
	$(DOTNET) run --project SPICA.WinForms -c $(CONFIG)

clean:
	$(DOTNET) clean SPICA.sln -c $(CONFIG)

.PHONY: all install run clean
