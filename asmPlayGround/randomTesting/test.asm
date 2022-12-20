section .text
global _main

_main:
    mov rax, 13
    mov rcx, 5
    xor rdx, rdx
    idiv rcx
    mov rdi, rax;->result rdx->remainder
    mov rax, 60 ;sys_exit
    syscall
