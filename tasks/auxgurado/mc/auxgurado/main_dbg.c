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
  uart_send_str("fuck0" CRLF, UART3); 
  zero_buffer(Question, 32);
  uart_send_str("fuck1" CRLF, UART3); 
  uart_read_str(Question, 32, UART3);
  uart_send_str("fuck" CRLF, UART3);  
  uart_send_str(Question, UART3);
  uart_send_str(CRLF, UART3); 
  ASMCALL(do_guess, main_cont);
}

char itoa_c[64];   
char *itoa(uint32_t i)
{
  int j;
  j = 64;
  do
  {
    itoa_c[--j] = '0' + (i % 10);
    i /= 10;
  }
  while (i > 0);
  return itoa_c + j;
}

char *qtoa(unsigned long long i)
{
  int j;
  j = 64;
  do
  {
    itoa_c[--j] = "0123456789abcdef"[i % 16];
    i /= 16;
  }
  while (i > 0);
  return itoa_c + j;
}

void main_cont()
{      
uart_send_str(itoa(42), UART3);  
  uart_send_str(CRLF, UART3);               
  uart_send_str(itoa(Selection), UART3); 
  uart_send_str(CRLF, UART3);               
  uart_send_str(itoa((uint32_t)Answer), UART3);  
  uart_send_str(CRLF, UART3);
  uart_send_str(Answer, UART3);
  uart_send_str(CRLF, UART3); 
  uart_send_str(CRLF, UART3);
  uart_send_str(qtoa(Debug0), UART3); 
  uart_send_str(CRLF, UART3); 
  uart_send_str(qtoa(Debug1), UART3); 
  uart_send_str(CRLF, UART3);
  uart_send_str(qtoa(Debug2), UART3); 
  uart_send_str(CRLF, UART3);
  main_loop();
}       