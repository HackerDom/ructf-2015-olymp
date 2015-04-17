import random
import string

emojis = "😊😃😉😆😜😋😍😎😒😏😔😢😭😩😨😐😌😄😇😰😲😳😷😚😕😯😦😵😠😡😝😴😘😟😬😶😪😀😥😛😖😤😣😧😑😅😮😞😙😓😁😱"

alphabet = list(emojis)
random.shuffle(alphabet)
mapping = dict(zip(string.ascii_lowercase, alphabet))

with open('cryptogram', 'wt', encoding='utf-8') as output:
	with open('plaintext.txt', 'rt') as plaintext_file:
		plaintext = plaintext_file.read()
	for ch in list(plaintext):
		output.write(mapping[ch] if ch in mapping else ch)
