nasm -felf64 -p macros.asm -p syscalls.asm -o temp.o temp.asm
ld -L . -rpath \$ORIGIN -o temp -ladding temp.o -e _main --dynamic-linker /lib64/ld-linux-x86-64.so.2
