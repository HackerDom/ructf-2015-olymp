from bottle import route, request, view, static_file, run
import serial

DEBUG = False
HOST = "0.0.0.0"
PORT = 16780

COMPORT = 4
BAUDRATE = 38400

@route('/static/<filename>')
def server_static(filename):
    return static_file(filename, root='static')


@route('/')
@view('index')
def answer():
    return {'answer': ''}


@route('/', method='POST')
@view('index')
def do_answer():
    question = request.forms.get('q')
    if not check_question(question):
        answer = "Please write in ASCII and don't get too long."
    else:
        answer = get_answer(question)
    #print(question)
    return {'answer': answer}


def check_question(question):
    return 0 < len(question) < 256 and all(31 < ord(c) < 128 for c in question)


def get_answer(question):

    ser = serial.Serial(
        port=COMPORT,
        baudrate=BAUDRATE,
        parity=serial.PARITY_NONE,
        stopbits=serial.STOPBITS_ONE,
        bytesize=serial.EIGHTBITS
    )

    answer = 'I cannot answer you now'
    try:
        ser.write(question.encode('utf-8') + b'\r')
        line = ser.readline()

        if len(line) > 0:
            answer = line
    finally:
        ser.close()

    return answer

run(host=HOST, port=PORT, debug=DEBUG)
