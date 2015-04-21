#include <HDL51001_ccf.h>
#include <uart.h> 
#include <wdt.h> 
#include "magic.h"

#define CRLF "\r\n"
#define ASMCALL(x, y) Return=y;(x)();

void zero_buffer(char *buffer, int length)
{
  int i;
  for (i = 0; i < length; i++)
    buffer[i] = 0;
}

void uart_read_str(char *buffer, int length, UART_TypeDef *uart)
{
  int i;
  
  for (i = 0; i < length; i++)
  {
    while (!UART_NEW_DATA(uart)) ;
    buffer[i] = UART_GET_BYTE(uart);
    if (buffer[i] == '\r')
    {
      buffer[i] = 0;
      break;
    }
  }
}

void uart_configure()
{
  UART_InitTypeDef UART_InitStructure;
  UART_InitStructure.BaudRate = 38400; //set baudrate
  UART_InitStructure.TypeParity = 0x00000000; //parity control type
  UART_InitStructure.Parity = 0x00000000; //enable parity control
  UART_InitStructure.FlowControl = 0x00000000; //enable cts/rts
  UART_InitStructure.Mode = 0x00000003; //rx enable - 1 bit, tx enable - 2 bit (rx + tx en)
  
	GPIOD->BPS = 0x00000F00;		 //alternative port function for uart3
	uart_init(UART3, &UART_InitStructure);
}

void main_loop();
void main_cont();

void main()
{        
  WDT_OFF;

	uart_configure();               
  
  main_loop();
}   

void main_loop()
{          
  zero_buffer(Question, 32);
  uart_read_str(Question, 32, UART3);
  ASMCALL(do_guess, main_cont);
}

void main_cont()
{                  
  uart_send_str(Answer, UART3);
  uart_send_str(CRLF, UART3); 
  main_loop();
}       