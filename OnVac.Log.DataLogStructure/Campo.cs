using System;

namespace Onit.OnAssistnet.OnVac.Log.DataLogStructure
{
	/// <summary>
	/// Descrizione di riepilogo per Campo.
	/// </summary>
	[Serializable]
	public class Campo
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="valore"></param>
		public Campo(string campo, string valore)
		{
			this._CampoDB = "";
			this._ValoreVecchio = "";
			this._ValoreNuovo = "";
			this._CampoDB = campo;
			this._ValoreVecchio = valore;
		}
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="valore"></param>
        /// <param name="valoreNuovo"></param>
		public Campo(string campo, string valore, string valoreNuovo) : this(campo, valore)
		{
			this._ValoreNuovo = valoreNuovo;
		}

        #endregion

        #region Properties
        
        /// <summary>
        /// Codice progressivo del campo
        /// </summary>
        public long? CodiceCampo { get; set; }

        private string _CampoDB;
        /// <summary>
        /// 
        /// </summary>
		public string CampoDB
		{
			get { return this._CampoDB; }
			set { this._CampoDB = value; }
		}

        private string _ValoreNuovo;
        /// <summary>
        /// 
        /// </summary>
		public string ValoreNuovo
		{
			get { return this._ValoreNuovo; }
			set { this._ValoreNuovo = value; }
		}

        private string _ValoreVecchio;
        /// <summary>
        /// 
        /// </summary>
		public string ValoreVecchio
		{
			get { return this._ValoreVecchio; }
			set { this._ValoreVecchio = value; }
        }

        #endregion
    }
}
