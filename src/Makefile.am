ASSEMBLY = njb-sharp.dll
TESTASM = njb-test.exe
INCLUDES = $(NJB_CFLAGS)

njbdir = $(prefix)/lib/njb-sharp
njb_DATA = $(ASSEMBLY) 
njb_LTLIBRARIES = libnjbglue.la

noinst_DATA = $(TESTASM)

libnjbglue_la_LDFLAGS = -avoid-version -module
libnjbglue_la_SOURCES = \
    $(srcdir)/njb-glue.c
libnjbglue_la_LIBADD = $(NJB_LIBS)

ASSEMBLY_SOURCES = \
    $(srcdir)/Discoverer.cs \
    $(srcdir)/Device.cs \
    $(srcdir)/Song.cs \
    $(srcdir)/SongFrame.cs \
    $(srcdir)/Global.cs \
    $(srcdir)/Utility.cs \
    $(srcdir)/Revision.cs

TESTASM_SOURCES = \
    $(srcdir)/NjbTest.cs

all: $(ASSEMBLY) $(TESTASM)

$(ASSEMBLY): $(ASSEMBLY_SOURCES)
	$(MCS) $(MCS_FLAGS) -target:library -out:$@ $(ASSEMBLY_SOURCES)

$(TESTASM): $(TESTASM_SOURCES)
	$(MCS) $(MCS_FLAGS) -out:$@ -r:$(ASSEMBLY) $(TESTASM_SOURCES)

run:
	$(MONO) $(TESTASM)

EXTRA_DIST = $(ASSEMBLY_SOURCES) $(TESTASM_SOURCES)

CLEANFILES = $(ASSEMBLY) $(TESTASM)
DISTCLEANFILES = Makefile.in

