extern add

SECTION .text
global _main

_main:
    mov rdi, 3
    mov rsi, 5
    call add
    exit rax