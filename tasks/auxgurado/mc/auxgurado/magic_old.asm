.data
	
	.global Messages
	.align 4
	Messages:
		.long M0
		.long M1
		.long M2
		.long M3
		.long M4
		.long M5
		
	Question: .ascii "!!!!!!!!!0y!B!!A\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00"

.bss

	;.global Question
	;Question: .space 32
	
	.global Answer
	Answer: .long
	
	.global Return
	Return: .long
	
	.global Selection
	Selection: .long
	
	.global Debug0
	Debug0: .quad
	
	.global Debug1
	Debug1: .quad
	
	.global Debug2
	Debug2: .quad

.text

.global do_guess
do_guess:
	
	jmp do_guess.1
	
	rdq Question
	rdq Question + 8
	rdq Question + 16
	rdq Question + 24
	
	xorq @4, @3
	xorq @1, @3
	xorq @1, @3
	setq #0, @1
	
complete

do_guess.1:

	jmp do_guess.2
	
	getq #0
	getq 0
	patch @1, @2
	pack @2, @3
	subl @1, @2
	setq #0, @1
	
	wrl @2, Debug1

complete

do_guess.2:

	jmp do_guess.3

	getl 0xB2D060ED
	mull @1, #0
	setq #0, @1
	
	wrl @2, Debug0

complete

do_guess.3:

	getl #0
	setq #1, @1
	subl @2, 0x30000000
	setq #2, @1
	andq @2, @2
	
	js @1, do_guess.m0
	jns @2, do_guess.4

complete

do_guess.4:

	getl #0
	subl @1, 0x60000000
	andq @1, @1
	
	js @1, do_guess.m1
	jns @2, do_guess.5

complete

do_guess.5:

	getl #0
	subl @1, 0x90000000
	andq @1, @1
	
	js @1, do_guess.m2
	jns @2, do_guess.6

complete

do_guess.6:

	getl #0
	subl @1, 0xC0000000
	andq @1, @1
	
	js @1, do_guess.m3
	jns @2, do_guess.7

complete

do_guess.7:

	getl #0
	subl @1, 0xDEADBEA7
	andq @1, @1
	
	jne @1, do_guess.m4
	je @2, do_guess.m5

complete

do_guess.m0:

	jmp do_guess.ret
	
	rdl Messages
	wrl @1, Answer
	getl 0
	wrl @1, Selection
	
complete

	
do_guess.m1:

	jmp do_guess.ret
	
	rdl Messages + 4
	wrl @1, Answer
	getl 1
	wrl @1, Selection
	
complete

	
do_guess.m2:

	jmp do_guess.ret
	
	rdl Messages + 8
	wrl @1, Answer
	getl 2
	wrl @1, Selection
	
complete

	
do_guess.m3:

	jmp do_guess.ret
	
	rdl Messages + 12
	wrl @1, Answer
	getl 3
	wrl @1, Selection
	
complete

	
do_guess.m4:

	jmp do_guess.ret
	
	rdl Messages + 16
	wrl @1, Answer
	getl 4
	wrl @1, Selection
	
complete

	
do_guess.m5:

	jmp do_guess.ret
	
	rdl Messages + 20
	wrl @1, Answer
	getl 5
	wrl @1, Selection
	
complete

do_guess.ret:

	rdl Return
	
	jmp @1
	
complete

.data

	M0: .asciz "Yes"
	M1: .asciz "No"
	M2: .asciz "Maybe"
	M3: .asciz "I don't know"
	M4: .asciz "Probably"
	M5: .asciz "XXXXX"