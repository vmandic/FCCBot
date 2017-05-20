using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using Microsoft.ProjectOxford.Emotion.Contract;
using System.Web;

namespace FamousCroatianConfessionBot.Model {
	public class DataModel {

		static string PersonsDataPath = VirtualPathUtility.ToAbsolute( "~/persons.bson");
		public static HashSet<Person> Persons = new HashSet<Person>();

		public static void SaveToFile() {
			using ( MemoryStream ms = new MemoryStream() )
			using ( BsonWriter writer = new BsonWriter( ms ) ) {
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize( writer, Persons );
				File.WriteAllText( PersonsDataPath, Convert.ToBase64String( ms.ToArray() ) );
			}
		}

		public static void LoadFromFile() {
			if ( File.Exists( PersonsDataPath ) ) {
				using ( MemoryStream ms = new MemoryStream( Convert.FromBase64String( File.ReadAllText( PersonsDataPath ) ) ) )
				using ( BsonReader reader = new BsonReader( ms ) ) {
					JsonSerializer serializer = new JsonSerializer();
					Persons = serializer.Deserialize<HashSet<Person>>( reader );
				}
			}
		}

		public Person GetPerson( string slackName ) {

			var person = Persons.FirstOrDefault( x => x.SlackName == slackName );

			if ( person == null ) {
				person = new Person( slackName );
				Persons.Add( person );
			}

			return person;
		}

		public class Person {
			public Person( string slackName ) { SlackName = slackName; }

			public string SlackName { get; set; }
			public List<Confession> Confessions { get; set; } = new List<Confession>();
		}

		#region Reckognition

		public class FaceVerification {

		}

		public class FaceEmotion {
			public Emotion Emotion { get; set; }
		}

		#endregion

		public class Confession {
			public FaceVerification FaceVerificationResult { get; set; }
			public FaceEmotion FaceEmotionResult { get; set; }
			public DateTime DateRequested { get; set; } = DateTime.Now;
			public string Response { get; set; }
		}
	}
}