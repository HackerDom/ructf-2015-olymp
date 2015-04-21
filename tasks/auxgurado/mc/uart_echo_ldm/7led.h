#ifndef _7LED_H
#define _7LED_H

void led7_char(unsigned char c);
void led7_4block(const char *block, unsigned int duration);
void led7_message(const char *message, int times, int speed);

#endif