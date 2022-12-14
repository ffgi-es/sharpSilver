SECTION .text
global add:function add.end-add

add:
    mov rax, rdi
    add rax, rsi
    add rax, 2
    ret
.end: