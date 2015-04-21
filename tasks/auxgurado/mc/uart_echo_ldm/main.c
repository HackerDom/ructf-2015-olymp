#include <HDL51001_ccf.h>
#include <uart.h> 
#include <wdt.h>
#include "7led.h"

#define CRLF "\r\n"

void set_buffer(char *buffer, int length, char value)
{
  int i;
  for (i = 0; i < length; i++)
    buffer[i] = value;
}

void uart_read(char *buffer, int length, UART_TypeDef *uart)
{
  int i;
  for (i = 0; i < length; i++)
  {
    while (!UART_NEW_DATA(uart)) ;
    buffer[i] = UART_GET_BYTE(uart);
  }
}

void uart_read_str_echo(char *buffer, int length, UART_TypeDef *uart)
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
    
    if (buffer[i] == 8 || buffer[i] == 127)
    {
      if (i > 0)
      {
        UART_SEND_BYTE(buffer[i], uart);
        buffer[i - 1] = buffer[i] = 0;
        i -= 2;
      }
      else
      {
        buffer[i] = 0;
        i--;
      }
    }
    else
    {
      UART_SEND_BYTE(buffer[i], uart);
    }
  }
}

void format_led7_message(char *buffer, const char *message)
{
  set_buffer(buffer, 4, ' ');
  buffer += 4;
  while (*message)
    *(buffer++) = *(message++);
  set_buffer(buffer, 4, ' ');
  buffer += 4;    
  *buffer = 0;
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

void main()
{   
  char buffer[16];
  char led7_buffer[64];
     
  WDT_OFF;
	GPIOB->DIR = ((uint32_t)0xFFB00000);

	uart_configure();
   
  led7_message("echo", 1, 1000);                
  uart_send_str("Welcome to Multiclet Echo Server (LDM)!" CRLF CRLF, UART3);
  while (1)
  {
    set_buffer(buffer, 16, 0);
    uart_send_str("> ", UART3);
    uart_read_str_echo(buffer, 16, UART3);
    uart_send_str(CRLF, UART3); 
    uart_send_str(buffer, UART3); 
    uart_send_str(CRLF, UART3);
    format_led7_message(led7_buffer, buffer);
    led7_message(led7_buffer, 1, 30);
  }
}
