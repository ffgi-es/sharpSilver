nasm -felf64 -o test.o test.asm
ld -e _main -o test test.o

./test
