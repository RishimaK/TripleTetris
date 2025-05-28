// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("VOiM2M5ZpYI0REDjf/W0KWI+BvEIi4WKugiLgIgIi4uKLs04vXmHGrNgwA4Om+wrP//KqBL4ExC8wXXvueJDA6pRGk8oDhWMeNYF5akppprqdqRPxHShAQ8rQg4fJNlSr5Mrn+CVLD3YnnDTLr57E9O7I1sGHgsdO2J4ljcLbWcTLGVWRI33teqz2JjUjb4l0bcLk+rrZiQqueWwT4PZ9LoIi6i6h4yDoAzCDH2Hi4uLj4qJATEuJywjMRNW5Jh1uK6DR8A7QbkTIEp3DZkSppVMOW4Opm8hH7L1G7plR4BqLV1BHagnruJ3z2cW2cD0ujDb0PdPhQoxIACmGHdvKUDTcnUNHl7ZJbG/CvHWhGmZLLYiHhgrqRH5EdrAtJu5jYiJi4qL");
        private static int[] order = new int[] { 12,1,5,5,11,7,13,11,12,13,11,13,12,13,14 };
        private static int key = 138;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
