#define _GNU_SOURCE
#include <sys/sysinfo.h>
#include <dlfcn.h>
#include <stddef.h>

int sysinfo(struct sysinfo *info) {
    int (*real_sysinfo)(struct sysinfo *) = dlsym(RTLD_NEXT, "sysinfo");
    if (!real_sysinfo) {
        return -1;
    }
    int ret = real_sysinfo(info);
    if (ret == 0) {
        // Reportar un minimo de 2048 MB (2 GB) de RAM para saltar el chequeo
        info->totalram = 2048ULL * 1024 * 1024 / info->mem_unit;
        info->freeram = info->totalram;
    }
    return ret;
}
