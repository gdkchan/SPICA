# GNU Make makefile for building with Mono
# (see README.mono.md)

MONO    = mono
MSBUILD = msbuild

#CONFIG = Debug
CONFIG = Release

SPICA_EXE = SPICA.WinForms/bin/$(CONFIG)/SPICA.WinForms.exe

# Note: The Mono compiler seems to dump some detritus by default in
# /tmp, so we set TMPDIR to a more suitable location

$(SPICA_EXE): SPICA.sln
	mkdir -p tmp
	TMPDIR=$(pwd)/tmp $(MSBUILD) /p:Configuration=$(CONFIG) $<

install: $(SPICA_EXE)
	rm -rf spica-install
	mkdir spica-install
	cp -np */bin/$(CONFIG)/*.dll spica-install
	cp -np */bin/$(CONFIG)/*.exe spica-install
	mv -n spica-install/SPICA.WinForms.exe spica-install/SPICA.exe

run: $(SPICA_EXE)
	$(MONO) $<

run-install: spica-install/SPICA.exe
	$(MONO) $<

clean:
	rm -rf SPICA*/bin SPICA*/obj
	rm -rf spica-install
	rm -rf tmp

.PHONY: clean install run run-install

# EOF
