using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SharperSave
{
    /// <summary>
    /// This class contains methods to check integrity and hide data.
    /// </summary>
    public static class SaveIntegrityUtility 
    {
        /// <summary>
        /// Shuffles a list of bytes based on an integer seed.
        /// </summary>
        /// <param name="originalBytes">The original bytes to shuffle.</param>
        /// <param name="seed">Seed to be used in the random number generator.</param>
        /// <returns>A list with shuffled bytes.</returns>
        public static List<byte> ShuffleBytes(List<byte> originalBytes, int seed)
        {
            List<int> order = GetOrder(originalBytes.Count, seed);
            byte[] result = new byte[order.Count];
            
            for (int i = 0; i < originalBytes.Count; i++)
            {
                result[i] = originalBytes[order[i]];
            }

            return result.ToList();
        }

        /// <summary>
        /// Unshuffles a list bringing back the original bytes positions.
        /// </summary>
        /// <param name="shuffledBytes">The shuffled bytes list.</param>
        /// <param name="seed">The seed was used to shuffle.</param>
        /// <returns></returns>
        public static List<byte> UnshuffleBytes(List<byte> shuffledBytes, int seed)
        {
            List<int> order = GetOrder(shuffledBytes.Count, seed);
            byte[] result = new byte[order.Count];

            for (int i = 0; i < shuffledBytes.Count; i++)
            {
                result[order[i]] = shuffledBytes[i];
            }

            return result.ToList();
        }

        /// <summary>
        /// Gets a SHA-256 from the content with a salt.
        /// </summary>
        /// <param name="content">The content to compute the hash.</param>
        /// <param name="salt">A salt to be added</param>
        /// <returns>A SHA-256 hash.</returns>
        public static string GetStringHash(string content, string salt)
        {
            string result = "";
            byte[] hashBytes;
            using (SHA256 hasher = SHA256.Create())
            {
                hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(salt + content));

                result = BytesToHexadecimal(hashBytes);
            }
            return result;
        }

        private static string BytesToHexadecimal(byte[] bytesToConvert)
        {
            string result = "";

            for (int i = 0; i < bytesToConvert.Length; i++)
            {
                result += bytesToConvert[i].ToString("x2");
            }
            return result;
        }
        /// <summary>
        /// Generates an order for the bytes.
        /// </summary>
        /// <param name="bytesSize">The size of the bytes list.</param>
        /// <param name="seed">Seed to be  used in the random number generator.</param>
        /// <returns>A order of shuffled numbers.</returns>
        private static List<int> GetOrder(int bytesSize, int seed)
        {
            System.Random random = new System.Random(seed);
            var order = new List<int>();
            var possibleNumbers = new List<int>();

            for (int i = 0; i < bytesSize; i++)
            {
               possibleNumbers.Add(i);
            }

            while (possibleNumbers.Count > 0)
            {
                int selectedNumber = random.Next(possibleNumbers.Count);
                order.Add(possibleNumbers[selectedNumber]);
                possibleNumbers.RemoveAt(selectedNumber);
            }
            return order;
        }
    }
}