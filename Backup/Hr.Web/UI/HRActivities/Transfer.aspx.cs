﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ASL.DATA;
using ASL.Hr.BLL;
using ASL.Hr.DAO;
using ASL.STATIC;
using ASL.Web.Framework;

namespace Hr.Web.UI.HRActivities
{
    public partial class Transfer : PageBase
    {
        PromotionManager ManagerPromotion = new PromotionManager();
        #region Sesson Variable
        private CustomList<ASL.Hr.DAO.TransferAndPromotionHistory> _CurrentPosition
        {
            get
            {
                if (ViewState["Transfer_CurrentPosition"] == null)
                    return new CustomList<ASL.Hr.DAO.TransferAndPromotionHistory>();
                else
                    return (CustomList<ASL.Hr.DAO.TransferAndPromotionHistory>)ViewState["Transfer_CurrentPosition"];
            }
            set
            {
                ViewState["Transfer_CurrentPosition"] = value;
            }
        }
        private CustomList<ASL.Hr.DAO.TransferAndPromotionHistory> _TransferSavedList
        {
            get
            {
                if (ViewState["Transfer_TransferSavedList"] == null)
                    return new CustomList<ASL.Hr.DAO.TransferAndPromotionHistory>();
                else
                    return (CustomList<ASL.Hr.DAO.TransferAndPromotionHistory>)ViewState["Transfer_TransferSavedList"];
            }
            set
            {
                ViewState["Transfer_TransferSavedList"] = value;
            }
        }
        private CustomList<ASL.Hr.DAO.EmployeeSalary> _CurrentSalary
        {
            get
            {
                if (Session["PromotionIncrement_CurrentSalary"] == null)
                    return new CustomList<ASL.Hr.DAO.EmployeeSalary>();
                else
                    return (CustomList<ASL.Hr.DAO.EmployeeSalary>)Session["PromotionIncrement_CurrentSalary"];
            }
            set
            {
                Session["PromotionIncrement_CurrentSalary"] = value;
            }
        }
        private CustomList<EntityList> _entityList
        {
            get
            {
                if (ViewState["Transfer_entityList"] == null)
                    return new CustomList<EntityList>();
                else
                    return (CustomList<EntityList>)ViewState["Transfer_entityList"];
            }
            set
            {
                ViewState["Transfer_entityList"] = value;
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                String eventTarget = Request["__EVENTTARGET"].IsNullOrEmpty() ? String.Empty : Request["__EVENTTARGET"];
                InitializeSession();
                InitializeControls();
            }
            else
            {
                Page.ClientScript.GetPostBackEventReference(this, String.Empty);
                String eventTarget = Request["__EVENTTARGET"].IsNullOrEmpty() ? String.Empty : Request["__EVENTTARGET"];
                if (eventTarget.Equals("SearchEmployee"))
                {
                    String EmployeeCode = Request["__EVENTARGUMENT"].IsNullOrEmpty() ? String.Empty : Request["__EVENTARGUMENT"];
                    if (EmployeeCode.IsNullOrEmpty())
                        EmployeeCode = ((TextBox)this.Header1.FindControl("txtSearch")).Text.ToString();
                    _CurrentPosition = ManagerPromotion.GetAllExistingInfoForPromotion(EmployeeCode, Convert.ToInt32(ASL.Hr.DAO.enumsHr.enumEntitySetup.TransferCriteria));
                    if (_entityList.Count == 0 && _CurrentPosition.Count == 0) return;
                    foreach (EntityList M in _entityList)
                    {
                        TransferAndPromotionHistory obj = _CurrentPosition.Find(f => f.EntityName == M.EntityName);
                        TextBox txt = (TextBox)Panel1.FindControl("txtCurrent" + M.EntityName.ToString());
                        txt.Text = obj.PreHKEntryName.ToString();

                        DropDownList ddl = (DropDownList)Panel1.FindControl("ddlPost" + M.EntityName.ToString());
                        ddl.Text = obj.PreHKEntryID.ToString();
                    }
                }
            }
        }
        #region All Methods
        private void InitializeSession()
        {
            try
            {
                _CurrentPosition = new CustomList<TransferAndPromotionHistory>();
                _CurrentSalary = new CustomList<EmployeeSalary>();
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        public void ClearControls()
        {
            try
            {
                txtEmployeeName.Text = string.Empty;
                txtDesignation.Text = string.Empty;
                txtStaffCategory.Text = string.Empty;
                txtDOJ.Text = string.Empty;
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        override protected void OnInit(EventArgs e)
        {
            string strTable = string.Format("<table><tr style='font-size: 15px;'><td style='border: 1px solid #B6B3AE;padding:5px 30px 5px 30px'>Entity</td><td style='border: 1px solid #B6B3AE; padding:5px 30px 5px 30px'>Current Position</td><td style='border: 1px solid #B6B3AE; padding:5px 30px 5px 30px'>Post Position</td></tr></table>");
            Panel1.Controls.Add(new LiteralControl(strTable));
            Panel1.Controls.Add(new LiteralControl("<div style='float:left; width:100%;font-size: 13px '>"));
            Panel1.Controls.Add(new LiteralControl("<div style='float:left;width:15%'>"));
            _entityList = EntityList.GetPromotionHiararchy(Convert.ToInt32(ASL.Hr.DAO.enumsHr.enumEntitySetup.TransferCriteria));
            int c = 0;
            foreach (EntityList M in _entityList)
            {
                Label lb;
                lb = new Label();
                lb.ID = "lvl" + M.EntityName.ToString();
                lb.Text = M.EntityName;
                lb.Width = 70;
                lb.CssClass.PadLeft(10);
                Panel1.Controls.Add(lb);
                Panel1.Controls.Add(new LiteralControl("<br/><br/>"));

            }
            Panel1.Controls.Add(new LiteralControl("</div>"));
            Panel1.Controls.Add(new LiteralControl("<div style='float:left;width:30%'>"));
            foreach (EntityList M in _entityList)
            {
                TextBox txt;
                txt = new TextBox();
                txt.ID = "txtCurrent" + M.EntityName.ToString();
                txt.Width = 100;
                txt.Attributes.Add("class", "txtwidth93px");
                txt.CssClass.PadLeft(1);
                Panel1.Controls.Add(txt);
                Panel1.Controls.Add(new LiteralControl("<br/><br/>"));
            }
            Panel1.Controls.Add(new LiteralControl("</div>"));
            Panel1.Controls.Add(new LiteralControl("<div style='float:left;width:30%'>"));
            foreach (EntityList M in _entityList)
            {
                DropDownList ddl;
                ddl = new DropDownList();
                ddl.ID = "ddlPost" + M.EntityName.ToString();
                ddl.DataSource = HouseKeepingValue.GetAllHouseKeepingValueForDropdown(M.EntityName);
                ddl.DataTextField = "HkName";
                ddl.DataValueField = "HkID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                ddl.SelectedIndex = 0;
                ddl.Width = 100;
                ddl.Attributes.Add("class", "txtwidth93px");
                ddl.CssClass.PadLeft(1);
                Panel1.Controls.Add(ddl);
                Panel1.Controls.Add(new LiteralControl("<br/><br/>"));
            }
            Panel1.Controls.Add(new LiteralControl("</div>"));
            Panel1.Controls.Add(new LiteralControl("</div>"));
            base.OnInit(e);
        }
        public void SetDateFromObjToControl(string EmployeeCode)
        {
            CustomList<LeaveTransApproved> _RunnignEmpInfo = ManagerPromotion.GetLeaveEligibleEmp(EmployeeCode);
            if (_RunnignEmpInfo.Count != 0)
            {
                txtEmployeeName.Text = _RunnignEmpInfo[0].EmpName;
                txtDesignation.Text = _RunnignEmpInfo[0].Designation;
                txtDOJ.Text = _RunnignEmpInfo[0].DOJ.ToShortDateString();
                txtStaffCategory.Text = _RunnignEmpInfo[0].StaffCategory;
                imgGarment.ImageUrl = ResolveUrl(_RunnignEmpInfo[0].EmpPicture);
                hfEmpKey.Value = _RunnignEmpInfo[0].EmpKey.ToString();
            }
        }
        private void InitializeControls()
        {
            try
            {
                #region Transfer Type
                ddlTransferType.DataSource = ManagerPromotion.GetAllGen_LookupEnt(ASL.Hr.DAO.enumsHr.enumEntitySetup.TransferType);
                ddlTransferType.DataTextField = "ElementName";
                ddlTransferType.DataValueField = "ElementKey";
                ddlTransferType.DataBind();
                ddlTransferType.Items.Insert(0, new ListItem() { Value = "", Text = "" });
                #endregion
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        private void SetDataFromControlToObject()
        {
            try
            {
                _TransferSavedList = new CustomList<TransferAndPromotionHistory>();
                foreach (EntityList M in _entityList)
                {
                    ASL.Hr.DAO.TransferAndPromotionHistory TPH = new TransferAndPromotionHistory();
                    TPH.EmpKey = Convert.ToInt64(hfEmpKey.Value);
                    TransferAndPromotionHistory obj = _CurrentPosition.Find(f => f.EntityName == M.EntityName);
                    TPH.PreHKEntryID = obj.PreHKEntryID;
                    TPH.PreHKEntryName = obj.PreHKEntryName;
                    TPH.EntityID = obj.EntityID;
                    TPH.EntityName = obj.EntityName;
                    DropDownList ddl = (DropDownList)Panel1.FindControl("ddlPost" + M.EntityName.ToString());
                    TPH.CurrentHKEntryID = ddl.SelectedValue.ToInt();
                    TPH.CurrentHKEntryName = ddl.SelectedItem.Text;
                    TPH.EffectiveDate = txtEffectiveDate.Text.ToDateTime();
                    TPH.Type = ddlTransferType.SelectedValue.ToInt();
                    TPH.StatusType = "Transfer";
                    TPH.Remarks = txtRemarks.Text;
                    TPH.NextReviewDate = txtNextReviewDate.Text.ToDateTime();
                    TPH.AddedBy = CurrentUserSession.UserCode;
                    TPH.AddedDate = DateTime.Now;
                    _TransferSavedList.Add(TPH);
                }
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        #endregion
        #region Button Event
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetDataFromControlToObject();
                CustomList<ASL.Hr.DAO.TransferAndPromotionHistory> TransferList = _TransferSavedList;
                ManagerPromotion.SavePromotion(ref TransferList);
                ((PageBase)this.Page).SuccessMessage = (StaticInfo.SavedSuccessfullyMsg);
            }
            catch (SqlException ex)
            {
                this.ErrorMessage = (ExceptionHelper.getSqlExceptionMessage(ex));
            }
            catch (Exception ex)
            {
                this.ErrorMessage = (ExceptionHelper.getExceptionMessage(ex));
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {

        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                FormUtil.ClearForm(this, FormClearOptions.ClearAll, false);
            }
            catch (Exception ex)
            {

                throw (ex);
            }

        }
        #endregion
    }
}
