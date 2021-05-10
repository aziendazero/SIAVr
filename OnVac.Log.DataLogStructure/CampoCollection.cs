using System;
using System.Collections;

namespace Onit.OnAssistnet.OnVac.Log.DataLogStructure
{
	/// <summary>
	/// Descrizione di riepilogo per CampoCollection.
	/// </summary>
	[Serializable]
	public class CampoCollection: CollectionBase
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
        public Campo this[int index]
        {
            get { return (Campo)this.List[index]; }
            set { this.List[index] = value; }
        }

        #endregion
 
        #region Public 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
		public int Add(Campo campo)
		{
			if (this.AddCampo != null)
			{
				this.AddCampo(this, new AddCampoEventArgs(campo));
			}
			return this.List.Add(campo);
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
		public int AddIfChanged(Campo campo)
		{
			if (campo.ValoreNuovo != campo.ValoreVecchio)
			{
				if (this.AddCampo != null)
				{
					this.AddCampo(this, new AddCampoEventArgs(campo));
				}
				return this.List.Add(campo);
			}
			return -1;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
		public bool Contains(Campo campo)
		{
			return this.List.Contains(campo);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
		public int IndexOf(Campo campo)
		{
			return this.List.IndexOf(campo);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="campo"></param>
		public void Insert(int index, Campo campo)
		{
			if (this.AddCampo != null)
			{
				this.AddCampo(this, new AddCampoEventArgs(campo));
			}
			this.List.Insert(index, campo);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo"></param>
		public void Remove(Campo campo)
		{
			this.List.Remove(campo);
		}

        #endregion
	}
}
