REST API

CLIENT can t store all data so they nead to be stored in another place so we nead an api

API= application programing interface
REST= representational state tranfer
Important principles :
-Stateless
-Client-Server
-Uniform interface
-Layered system
-cacheable
-Code on demand

Resource path
RESOURCE =any object,document or thing that the api can receive or send to client

http://example.com/games
^ ^ ^
protocol domain resource

Client ->request-> Server
<-responce<-

HTTP METHODS (most common)

CRUD OPERATIONS
POST= Creates a new resource
GET=Retrieves the resource representation/state
PUT=Updates an existing resource
DELETE=Deletes a resource

DTO= is a contract between the client and server since it represents, a shared agreement about how data will be transferred and used.
Object-Relational Mapping (0/RM)= A technique for converting data between a relational
database and an object-oriented program

dotnet ef migrations add InitialCreate --output-dir Data\Migrations
