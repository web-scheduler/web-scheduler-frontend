function getOidToken(key) {
  var strJson = sessionStorage.getItem(key);
  if (strJson != null) {
    var json = JSON.parse(strJson);
    return json.id_token;
  }

  return null;
}
