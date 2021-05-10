using System;

namespace Onit.OnAssistnet.OnVac.Log.DataLogStructure
{
	/// <summary>
	/// Descrizione di riepilogo per Record.
	/// </summary>
	[Serializable]
	public class Record
    {
        #region Events

        /// <summary>
        /// 
        /// </summary>
		[field: NonSerialized]
		public event AddCampoEventHandler AddCampo;

        #endregion

        #region Properties

        private CampoCollection _Campi;
        /// <summary>
        /// Campi
        /// </summary>
        public CampoCollection Campi
        {
            get { return this._Campi; }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Costruttore
        /// </summary>
		public Record()
		{
			this._Campi = new CampoCollection();
			this._Campi.AddCampo += new AddCampoEventHandler(this.AddedCampo);
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
