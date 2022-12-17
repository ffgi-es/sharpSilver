section .text
global _main

_main:
    mov rdi, 4
    sub rdi, 3
    mov rax, 60 ;sys_exit
    syscall
