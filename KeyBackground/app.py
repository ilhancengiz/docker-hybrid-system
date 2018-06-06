from redis import Redis
import psycopg2
import time

def get_redis():
    if not hasattr(g, 'redis'):
        g.redis = Redis(host="redis", db=0, socket_timeout=5)
    return g.redis

def connectPostgre():
    conn_string = "postgres://postgres@db/postgres"
    # get a connection, if a connect cannot be made an exception will be raised here    
    try:
        conn = psycopg2.connect(conn_string)
        # conn.cursor will return a cursor object, you can use this cursor to perform queries
        cursor = conn.cursor()
        print ("Connected!\n")
        try:
            cursor.execute("""SELECT * from keyValues""")
            rows = cursor.fetchall()
            print(rows)
            for row in rows:
                print("Key: {0}, Value: {1}".format(row[0], row[1]))       
        except:
            print("I can't SELECT from keyValues")
    except:
        print("I am unable to connect to the database.")

def init():
    while True:
        connectPostgre()
        time.sleep(10)
        
if __name__ == "__main__":
    init()
