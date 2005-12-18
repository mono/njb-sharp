AC_INIT(README)
AC_CANONICAL_SYSTEM

MAJOR_VERSION=0
MINOR_VERSION=2
MICRO_VERSION=0
VERSION=$MAJOR_VERSION.$MINOR_VERSION.$MICRO_VERSION

AM_INIT_AUTOMAKE(njb-sharp, $VERSION)

AC_SUBST(MAJOR_VERSION)
AC_SUBST(MINOR_VERSION)
AC_SUBST(MICRO_VERSION)

AM_MAINTAINER_MODE
AC_PROG_CC
AM_PROG_LIBTOOL

PKG_CHECK_MODULES(NJB, libnjb >= 2.2.4)

AC_PATH_PROG(MONO, mono, no)
if test "x$MONO" = "xno"; then
    AC_MSG_ERROR([Can not find "mono" in your PATH])
fi

AC_PATH_PROG(MCS, mcs, no)
if test "x$MCS" = "xno"; then
    AC_MSG_ERROR([Can not find "mcs" in your PATH])
fi

AC_ARG_ENABLE(docs, AC_HELP_STRING([--disable-docs], [Do not build documentation]), with_docs=no, with_docs=yes)

if test "x$with_docs" = "xyes"; then
    AC_PATH_PROG(MONODOCER, monodocer, no)
    if test "x$MONODOCER" = "xno"; then
        AC_MSG_ERROR([You need to install monodoc])
    fi

    AC_PATH_PROG(MDASSEMBLER, mdassembler, no)
    if test "x$MDASSEMBLER" = "xno"; then
        AC_MSG_ERROR([You need to install mdassembler])
    fi

    DOCDIR=`$PKG_CONFIG monodoc --variable=sourcesdir`
    AC_SUBST(DOCDIR)

    AM_CONDITIONAL(BUILD_DOCS, true)
else
    AC_MSG_NOTICE([not building API documentation])
    AM_CONDITIONAL(BUILD_DOCS, false)
fi

expanded_libdir=`( case $prefix in NONE) prefix=$ac_default_prefix ;; *) ;; esac
		   case $exec_prefix in NONE) exec_prefix=$prefix ;; *) ;; esac
		   eval echo $libdir )`
AC_SUBST(expanded_libdir)

AC_OUTPUT([
    Makefile
    njb-sharp.pc
    src/Makefile
    src/AssemblyInfo.cs
	src/njb-sharp.dll.config
])

