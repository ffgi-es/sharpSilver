section .data

offset: db -11

SECTION .text
global add:function add.end-add

add:
    mov rax, rdi
    add rax, rsi
    add rax, [rel offset]
    ret
.end:
