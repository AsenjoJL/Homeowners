using Google.Cloud.Firestore;
using System;

namespace HOMEOWNER.Services
{
    /// <summary>
    /// Custom Firestore converters for DateTime and other types
    /// </summary>
    public static class FirestoreConverters
    {
        public static Timestamp ToFirestoreTimestamp(DateTime dateTime)
        {
            return Timestamp.FromDateTime(dateTime.ToUniversalTime());
        }

        public static DateTime FromFirestoreTimestamp(Timestamp timestamp)
        {
            return timestamp.ToDateTime();
        }

        public static DateTime? FromFirestoreTimestampNullable(Timestamp? timestamp)
        {
            return timestamp?.ToDateTime();
        }
    }
}

