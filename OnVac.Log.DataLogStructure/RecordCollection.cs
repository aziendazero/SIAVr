using System;
using System.Collections;

namespace Onit.OnAssistnet.OnVac.Log.DataLogStructure
{
	/// <summary>
	/// Descrizione di riepilogo per RecordCollection.
	/// </summary>
	[Serializable]
	public class RecordCollection: CollectionBase
    {
        #region Events

        /// <summary>
        /// 
        /// </summary>
		[field: NonSerialized]
		public event AddCampoEventHandler AddCampo;

        #endregion

        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Record this[int index]
        {
            get { return (Record)this.List[index]; }
            set { this.List[index] = value; }
        }

        #endregion

        #region Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
		public int Add(Record record)
		{
			record.AddCampo += new AddCampoEventHandler(this.AddedCampo);

			if (record.Campi.Count > 0)
			{
				foreach (Campo campo in record.Campi)
				{
					if (this.AddCampo != null)
					{
						this.AddCampo(this, new AddCampoEventArgs(campo));
					}
				}
			}
			return this.List.Add(record);
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
		public bool Contains(Record record)
		{
			return this.List.Contains(record);
		}
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
		public int IndexOf(Record record)
		{
			return this.List.IndexOf(record);
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="record"></param>
		public void Insert(int index, Record record)
		{
			record.AddCampo += new AddCampoEventHandler(this.AddedCampo);

			if (record.Campi.Count > 0)
			{
				foreach (Campo campo in record.Campi)
				{
					if (this.AddCampo != null)
					{
						this.AddCampo(this, new AddCampoEventArgs(campo));
					}
				}
			}
			this.List.Insert(index, record);
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
		public void Remove(Record record)
		{
			this.List.Remove(record);
		}

        #endregion

        #region Private

        private void AddedCampo(object sender, AddCampoEventArgs e)
        {
            if (this.AddCampo != null)
            {
                this.AddCampo(this, new AddCampoEventArgs(e.campo));
            }
        }

        #endregion
	}
}
