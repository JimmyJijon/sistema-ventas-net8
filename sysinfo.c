#define _GNU_SOURCE
#include <stdio.h>
#include <fcntl.h>
#include <dlfcn.h>
#include <string.h>
#include <sys/sysinfo.h>
#include <stdarg.h>

// Definir los tipos de las funciones originales
typedef int (*orig_open_t)(const char *pathname, int flags, ...);
typedef int (*orig_openat_t)(int dirfd, const char *pathname, int flags, ...);
typedef FILE* (*orig_fopen_t)(const char *pathname, const char *mode);
typedef int (*orig_sysinfo_t)(struct sysinfo *info);

// Hook para open()
int open(const char *pathname, int flags, ...) {
    orig_open_t orig_open = (orig_open_t)dlsym(RTLD_NEXT, "open");
    mode_t mode = 0;
    if (flags & O_CREAT) {
        va_list args;
        va_start(args, flags);
        mode = va_arg(args, mode_t);
        va_end(args);
    }
    if (pathname && strcmp(pathname, "/proc/meminfo") == 0) {
        return orig_open("/opt/mssql/lib/fake_meminfo", flags, mode);
    }
    return orig_open(pathname, flags, mode);
}

// Hook para openat()
int openat(int dirfd, const char *pathname, int flags, ...) {
    orig_openat_t orig_openat = (orig_openat_t)dlsym(RTLD_NEXT, "openat");
    mode_t mode = 0;
    if (flags & O_CREAT) {
        va_list args;
        va_start(args, flags);
        mode = va_arg(args, mode_t);
        va_end(args);
    }
    if (pathname && strcmp(pathname, "/proc/meminfo") == 0) {
        return orig_openat(dirfd, "/opt/mssql/lib/fake_meminfo", flags, mode);
    }
    return orig_openat(dirfd, pathname, flags, mode);
}

// Hook para fopen()
FILE* fopen(const char *pathname, const char *mode) {
    orig_fopen_t orig_fopen = (orig_fopen_t)dlsym(RTLD_NEXT, "fopen");
    if (pathname && strcmp(pathname, "/proc/meminfo") == 0) {
        return orig_fopen("/opt/mssql/lib/fake_meminfo", mode);
    }
    return orig_fopen(pathname, mode);
}

// Hook para sysinfo()
int sysinfo(struct sysinfo *info) {
    orig_sysinfo_t orig_sysinfo = (orig_sysinfo_t)dlsym(RTLD_NEXT, "sysinfo");
    if (!orig_sysinfo) {
        return -1;
    }
    int ret = orig_sysinfo(info);
    if (ret == 0) {
        // Reportar un minimo de 2048 MB (2 GB) de RAM para saltar el chequeo
        info->totalram = 2048ULL * 1024 * 1024 / info->mem_unit;
        info->freeram = info->totalram;
    }
    return ret;
}
