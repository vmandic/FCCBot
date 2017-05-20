using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using Microsoft.ProjectOxford.Emotion.Contract;

namespace FamousCroatianConfessionBot.Model {
	public static class DataModel {
		static Data _data = new Data();

		static string[] _helloMessages = new string[] {
			"Ako si tražila muškarca s dobrim stanjem na računu, moram ti priznati da je Isus otplatio sve moje dugove",
			"E oprosti, mislim da jedno tvoje rebro pripada meni",
			"Pazi da ne prekršiš petu Božju zapovijed, jer tvoja ljepota me ubija",
			"Primijetio sam da imaš lijepu Bibliju, mogli bi podcrtavati retke zajedno",
			"Izgledaš mi kao da slušaš Božje zapovijedi, e pa prva Božja zapovijed je plodite se i množite. Što kažeš?",
			"Jeste li možda za večeru? Evo, obećavam da neće biti posljednja",
			"Je li to ovdje tako vruće ili to gori duh sveti u tebi?"
		};

		public static string GetHelloMessage() {
			Random rng = new Random();
			return _helloMessages[rng.Next( 0, _helloMessages.Count() )];
		}



		public static void SaveToFile() {
			using ( MemoryStream ms = new MemoryStream() )
			using ( BsonWriter writer = new BsonWriter( ms ) ) {
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize( writer, _data );
				File.WriteAllText( _data.PersonsDataPath, Convert.ToBase64String( ms.ToArray() ) );
			}
		}

		public static void LoadFromFile() {
			if ( File.Exists( _data.PersonsDataPath ) ) {
				using ( MemoryStream ms = new MemoryStream( Convert.FromBase64String( File.ReadAllText( _data.PersonsDataPath ) ) ) )
				using ( BsonReader reader = new BsonReader( ms ) ) {
					JsonSerializer serializer = new JsonSerializer();
					_data.Persons = serializer.Deserialize<Data>( reader ).Persons;
				}
			}
		}

		public static Person GetPerson( string slackName ) {

			var person = _data.Persons.FirstOrDefault( x => x.SlackName == slackName );

			if ( person == null ) {
				person = new Person( slackName );
				_data.Persons.Add( person );
			}

			return person;
		}
		
		#region Graph

		public class Data {
			public string PersonsDataPath = HttpContext.Current.Server.MapPath( "~/App_Data/persons.bson" );
			public List<Person> Persons = new List<Person>();
		}
		public class Person {
			public Person( string slackName ) { SlackName = slackName; }

			public string SlackName { get; set; }
			public List<Confession> Confessions { get; set; } = new List<Confession>();
		}

		public class Confession {
			public FaceVerification FaceVerificationResult { get; set; }
			public FaceEmotion FaceEmotionResult { get; set; }
			public DateTime DateRequested { get; set; } = DateTime.Now;
			public string Response { get; set; }
		}

		#endregion

		#region Reckognition

		public class FaceVerification {

		}

		public class FaceEmotion {
			public Emotion Emotion { get; set; }
		}

		#endregion
	}
}