using System;
using System.Collections;

namespace Onit.OnAssistnet.OnVac.Log.DataLogStructure
{
	/// <summary>
	/// Descrizione di riepilogo per ArgomentoCollection.
	/// </summary>
	/// 
	[Serializable]
	public class ArgomentoCollection: CollectionBase
    {
        #region Private

        private Hashtable _hashTable;

        #endregion

        #region Properties

        /// <summary>
        /// Elemento di codice specificato
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        public Argomento this[string codice]
        {
            get { return (Argomento)this._hashTable[codice]; }
        }

        /// <summary>
        /// Elemento di indice specificato
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Argomento this[int index]
        {
            get { return (Argomento)this.List[index]; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
		public ArgomentoCollection()
		{
			this._hashTable = new Hashtable();
		}

        #endregion

        #region Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argomento"></param>
        /// <returns></returns>
		public int Add(Argomento argomento)
		{
			int add = -1;
			if (!this._hashTable.ContainsKey(argomento.Codice))
			{
				this._hashTable.Add(argomento.Codice, argomento);
				add = this.List.Add(argomento);
			}
			return add;
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argomento"></param>
        /// <returns></returns>
		public bool Contains(Argomento argomento)
		{
			return this.List.Contains(argomento);
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
		public bool Contains(string codice)
		{
			return this._hashTable.ContainsKey(codice);
		}

        /// <summary>
        /// 
        /// </summary>
		public int IndexOf(Argomento argomento)
		{
			return this.List.IndexOf(argomento);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="argomento"></param>
		public void Insert(int index, Argomento argomento)
		{
			if (!this._hashTable.ContainsKey(argomento.Codice))
			{
				this._hashTable.Add(argomento.Codice, argomento);
				this.List.Insert(index, argomento);
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argomento"></param>
		public void Remove(Argomento argomento)
		{
			this._hashTable.Remove(argomento.Codice);
			this.List.Remove(argomento);
        }

        #endregion
    }
}
