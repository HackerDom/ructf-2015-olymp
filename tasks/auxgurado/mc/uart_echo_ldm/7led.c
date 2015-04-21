//Uart test
#include <HDL51001_ccf.h>
#include <HDL50001_gpio.h>
#include "font.h"

#define S1(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 20)) : (GPIOB->OUT & (~(1 << 20))) )
#define S2(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 21)) : (GPIOB->OUT & (~(1 << 21))) )
#define S3(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 23)) : (GPIOB->OUT & (~(1 << 23))) )
#define S4(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 24)) : (GPIOB->OUT & (~(1 << 24))) )
#define DA(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 25)) : (GPIOB->OUT & (~(1 << 25))) )
#define DB(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 26)) : (GPIOB->OUT & (~(1 << 26))) )
#define DC(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 27)) : (GPIOB->OUT & (~(1 << 27))) )
#define DD(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 28)) : (GPIOB->OUT & (~(1 << 28))) )
#define DE(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 29)) : (GPIOB->OUT & (~(1 << 29))) )
#define DF(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 30)) : (GPIOB->OUT & (~(1 << 30))) )
#define DG(V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << 31)) : (GPIOB->OUT & (~(1 << 31))) )

#define DX(I,V) GPIOB->OUT = (((V & 0x01) == 1) ? (GPIOB->OUT | ((V & 0x01) << (25 + I))) : (GPIOB->OUT & (~(1 << (25 + I)))) )

#define led7_pause 5000

void led7_char(unsigned char c)
{
  int i;
  for (i = 0; i < 7; i++)
    DX(i, font[c] >> (6 - i));
}

void led7_4block(const char *block, unsigned int duration)
{
	int j;
	int k;
	for (j = 0; j < duration; j++)
	{
		S1(1);
		S2(1);
		S3(0);
		S4(1);
		led7_char(block[2]);
		for(k=0;k<led7_pause;k++);
		S1(1);
		S2(0);
		S3(1);
		S4(1);
		led7_char(block[1]);
		for(k=0;k<led7_pause;k++);
		S1(1);
		S2(1);
		S3(1);
		S4(0);
		led7_char(block[3]);
		for(k=0;k<led7_pause;k++);
		S1(0);
		S2(1);
		S3(1);
		S4(1);
		led7_char(block[0]);
		for(k=0;k<led7_pause;k++);
	}
}  

int _strlen(const char *s)
{
    int len = 0;
    while (*s) 
    {
      len++;
      s++;
    }
    return len;
}

void led7_message(const char *message, int times, int speed)
{
  uint32_t len, i, j;
  
  len = _strlen(message);
  for (j = 0; j < times; j++)
  {
    for (i = 0; i < len - 3; i++)
    {
      led7_4block(message + i, speed);
    }
  }
  led7_4block("    ", 1);
}













