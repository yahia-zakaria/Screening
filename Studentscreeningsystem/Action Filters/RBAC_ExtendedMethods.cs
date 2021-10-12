using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

//Get requesting user's roles/permissions from database tables...      
public static class RBAC_ExtendedMethods
{
    public static bool HasRole(this ControllerBase controller, string role)
    {
        bool bFound = false;
        var session = controller.ControllerContext.HttpContext.Session;
        try
        {
            //Check if the requesting user has the specified role...
            //bFound = new RBACUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasRole(role);
            bFound = new RBACUser(session["Username"].ToString()).HasRole(role);
        }
        catch { }
        return bFound;
    }

    public static bool HasRoles(this ControllerBase controller, string roles)
    {
        bool bFound = false;
        try
        {
            //Check if the requesting user has any of the specified roles...
            //Make sure you separate the roles using ; (ie "Sales Manager;Sales Operator"
            //bFound = new RBACUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasRoles(roles);
            bFound = new RBACUser(controller.ControllerContext.HttpContext.Session["Username"].ToString()).HasRoles(roles);
        }
        catch { }
        return bFound;
    }

    public static bool HasPermissionUser(this ControllerBase controller, string permission)
    {
        bool bFound = false;
        try
        {
            //Check if the requesting user has the specified application permission...
            //bFound = new RBACUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasPermission(permission);
            bFound = new RBACUser(controller.ControllerContext.HttpContext.Session["Username"].ToString()).HasPermission(permission);
        }
        catch { }
        return bFound;
    }

    public static bool IsAdminSys(this ControllerBase controller)
    {
        bool bIsSysAdmin = false;
        try
        {
            //Check if the requesting user has the System Administrator privilege...
            //bIsSysAdmin = new RBACUser(controller.ControllerContext.HttpContext.User.Identity.Name).IsSysAdmin;
            bIsSysAdmin = new RBACUser(controller.ControllerContext.HttpContext.Session["Username"].ToString()).IsSysAdmin;
        }
        catch { }
        return bIsSysAdmin;
    }
}