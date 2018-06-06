var express = require('express'),
    async = require('async'),
    pg = require("pg"),
    cookieParser = require('cookie-parser'),
    bodyParser = require('body-parser'),
    methodOverride = require('method-override'),
    app = express(),
    server = require('http').Server(app);

var server = app.listen(8081, function () {
   var host = server.address().address
   var port = server.address().port
   console.log("Example app listening at http://%s:%s", host, port);    
   CreateTableIfNotExists();
})


function updateKey(key, value) { 
   pg.connect('postgres://postgres@db/postgres', function(err, client, done) {
        if (err) throw err;
        console.log("Connected to db");
        client.query('UPDATE keyValues SET value = $2 WHERE key = $1', [key, value], function(err, result) {
            if (err) {
              console.error("Error performing query: " + err);
            } else {
              console.log("key updated!");
              client.end();
            }
          });
  });
}

function insertKey(key, value) {
    pg.connect('postgres://postgres@db/postgres', function(err, client, done) {
        if (err) throw err;
        console.log("Connected to db");
        client.query('INSERT INTO keyValues(key,value) VALUES($1,$2)', [key, value], function(err, result) {
            if (err) {
              console.error("Error performing query: " + err);
            } else {
              console.log("key inserted!");
              client.end();
            }
          });
  });
}

function getKeys(callback) {
    pg.connect('postgres://postgres@db/postgres', function(err, client, done) {
        if (err) throw err;
        console.log("Connected to db");
        client.query('SELECT key, value FROM keyValues', [], function(err, result) {
            if (err) {
              console.error("Error performing query: " + err);
            } else {
              console.log(result);  
              var dictinary = getKeysDictionary(result);
              callback(JSON.stringify(dictinary));
            }
         });
   });
}

function getKeysDictionary(result) {
  var keyPairs = {};
  result.rows.forEach(function (row) {
    keyPairs[row.key] = row.value;
  }); 
  return keyPairs;
}

function CreateTableIfNotExists()
{
    pg.connect('postgres://postgres@db/postgres', function(err, client, done) {
    if (err) throw err;
    console.log("Connected to db");
    client.query('CREATE TABLE IF NOT EXISTS keyValues(key VARCHAR(40) PRIMARY KEY, value VARCHAR(40) not null)', (err, res) => {
          if (err) throw err;
          console.log(res);
          client.end();
        });    
    //client.query('DELETE FROM keyValues', (err, res) => {
          //if (err) throw err;
          //console.log(res);
          //client.end();
        //});
  });
}

app.use(cookieParser());
app.use(bodyParser.urlencoded());
app.use(bodyParser.json());
app.use(methodOverride('X-HTTP-Method-Override'));
app.use(function(req, res, next) {
  res.header("Access-Control-Allow-Origin", "*");
  res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
  res.header("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, OPTIONS");
  next();
});

//app.use(express.static(__dirname + '/views'));

app.get('/', function (req, res) {
  getKeys(function(ret) {
      res.send('Greetings! Here keys -> : ' + ret);
  });
});


app.post('/newKey', function (req, res) {
   console.log("Key -> " + req.body.Key);
   console.log("Value -> " + req.body.Value);
   insertKey(req.body.Key, req.body.Value);
   res.send('Key Inserted!');
})

app.post('/updateKey', function (req, res) {
   console.log("Key -> " + req.body.Key);
   console.log("Value -> " + req.body.Value);
   updateKey(req.body.Key, req.body.Value);
   res.send('Key Updated!');
})
