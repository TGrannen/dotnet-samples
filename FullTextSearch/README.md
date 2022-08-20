# Full Text Search (ElasticSearch)

This project showcases the ability to use an [ElasticSearch](https://www.elastic.co/what-is/elasticsearch) instance to
perform full text searches on index objects. It will spin up two containers using
the [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) package to store and visualize simple
document data.

#### To visualize the values stored within the ElasticSearch index:

* Go to this address http://localhost:5601/app/home
* Analytics -> Discover -> Create Data View
* Enter 'my-index' into the text box

Packages include:

* [NEST](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/nest.html)
* [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet)

