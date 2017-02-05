  var BSON = require('bson');
  start();

  function start() {
    // Get the Long type
    var Long = BSON.Long;
    // Create a bson parser instance
    var bson = new BSON();

    // Serialize document
    //var doc = { long: Long.fromNumber(100) }




  var SocketRequestFormat = {
    method : 'testMethod1',
    id : 100,
    time : 200,
    param : {banana: "바나나", hong: "홍", monkey: "원숭이"}
  }

    // Serialize a document
    var data = bson.serialize(SocketRequestFormat)
    // De serialize it again
    var result = bson.deserialize(data)

    console.log("method = "+ result.method);
    console.log("id = "+ result.id);
    console.log("time = "+ result.time);

    for (var key in result.param)
     { console.log("key : " + key +", value : " + result.param[key]); }

  }


