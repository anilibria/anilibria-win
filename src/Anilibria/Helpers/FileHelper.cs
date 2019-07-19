namespace Anilibria.Helpers {

	/// <summary>
	/// File helper.
	/// </summary>
	public static class FileHelper {

		private static string[] m_FileSizes = { "B" , "KB" , "MB" , "GB" , "TB" };

		/// <summary>
		/// Get file size.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <returns>Readable size.</returns>
		public static string GetFileSize ( long size ) {
			var readableSize = size;
			int order = 0;
			while ( readableSize >= 1024 && order < m_FileSizes.Length - 1 ) {
				order++;
				readableSize = readableSize / 1024;
			}
			return readableSize + " " + m_FileSizes[order];
		}

	}

}
