# Nancy_web_api
  Jeigu projektas bus paleistas per Microsoft visual studio, prašau paleisti visual studio kaip administratorius "Run as administrator", 
  kitu atveju paleidžiant patį projektą per visual studio, gausite : "Nancy.Hosting.Self.AutomaticUrlReservationCreationFailureException"
  

 End-points:
Gauti klientu sąrašą:
- http://localhost:8000/getClientList
Gauti specifinį klientą:
- http://localhost:8000/getClient/id - prideti kliento numerį
Prideti naują klientą json užklausa:
- localhost:8000/postClient
json pavyzdys:
    {
        "name": "John",
        "age": 25,
        "comment": "Comment"
    }
Ištrinti klientą:
http://localhost:8000/deleteClient
- siųsti JSON užklausą su kliento id, kurį norite ištrinti

Atnaujinti klientą:
- http://localhost:8000/putClientUpdate - siųsti json užklausą su detalėmis ir norimo kliento Id. Pavyzdys:

    {
        "id": 1,
        "name": "Johnny",
        "age": 43,
        "comment": "yay, Im updated"
    }
