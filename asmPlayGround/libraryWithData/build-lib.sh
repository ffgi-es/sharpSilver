nasm -felf64 -o adding.o adding.asm
ld -shared -soname libadding.so -o libadding.so adding.o
