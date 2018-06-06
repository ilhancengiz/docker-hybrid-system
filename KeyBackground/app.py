from redis import Redis
import psycopg2
import time

def get_redis():
    if not hasattr(g, 'redis'):
        g.redis = Redis(host="redis", db=0, socket_timeout=5)
    print ("Connected to redis from python!")
    return g.redis

def connectPostgre():
    conn_string = "postgres://postgres@db/postgres"
    # get a connection, if a connect cannot be made an exception will be raised here    
    try:
        conn = psycopg2.connect(conn_string)
        # conn.cursor will return a cursor object, you can use this cursor to perform queries
        cursor = conn.cursor()
        print ("Connected to postgres from python!")
        try:
            cursor.execute("""SELECT * from keyValues""")
            rows = cursor.fetchall()
            for row in rows:
                print("Key: {0}, Value: {1}".format(row[0], row[1]))
            return rows				
        except:
            print("I can't SELECT from keyValues")
    except:
        print("I am unable to connect to the database.")

def setRedisValuesFromPostgres(redis, dbValues):
    # first delete all keys from redis
    for key in redis.scan_iter():
	    redis.delete(key)
    print ("All Redis values deleted!")	
	# set new values from db
    for row in dbValues:
        redis.set(row[0], row[1])		
    print ("Redis values updated from db values!")		
		
def init():
    while True:
        dbValues = connectPostgre()
		redis = get_redis()	
        setRedisValuesFromPostgres(redis, dbValues)		
        time.sleep(30)
        
if __name__ == "__main__":
    init()
