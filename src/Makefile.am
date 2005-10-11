ASSEMBLY = njb-sharp.dll
TESTASM = njb-test.exe
INCLUDES = $(NJB_CFLAGS)

DOC_UPDATER = @MONODOCER@ -delete
DOC_ASSEMBLER = @MDASSEMBLER@ --out njb-sharp-docs --ecma

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

if BUILD_DOCS
docdir = $(DOCDIR)
doc_DATA = \
    njb-sharp-docs.zip \
    njb-sharp-docs.tree \
    njb-sharp-docs.source
endif

njb_sharp_sources_in = AssemblyInfo.cs.in
njb_sharp_generated_sources = $(njb_sharp_sources_in:.in=)

all: $(ASSEMBLY) $(TESTASM)

$(ASSEMBLY): $(ASSEMBLY_SOURCES)
	$(MCS) $(MCS_FLAGS) -target:library -out:$@ $(ASSEMBLY_SOURCES) $(njb_sharp_generated_sources)

$(TESTASM): $(TESTASM_SOURCES)
	$(MCS) $(MCS_FLAGS) -out:$@ -r:$(ASSEMBLY) $(TESTASM_SOURCES)

update-docs: $(ASSEMBLY)
	$(DOC_UPDATER) -assembly:$(ASSEMBLY) -path:en

njb-sharp-docs.zip njb-sharp-docs.tree: $(srcdir)/en/*/*.xml
	$(DOC_ASSEMBLER) $(srcdir)/en

run:
	$(MONO) $(TESTASM)

EXTRA_DIST = \
    $(ASSEMBLY_SOURCES) \
    $(TESTASM_SOURCES) \
    $(srcdir)/en/*.xml \
    $(srcdir)/en/*/*.xml \
    njb-sharp-docs.source \
    AssemblyInfo.cs.in

CLEANFILES = $(ASSEMBLY) $(TESTASM)
DISTCLEANFILES = Makefile.in njb-sharp-docs.zip njb-sharp-docs.tree

