%macro exit 1
    mov rdi, %1
    mov rax, SYS_EXIT
    syscall
%endmacro