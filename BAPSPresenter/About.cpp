#include "StdAfx.h"
#include "About.h"

using namespace BAPSPresenter;

System::Void About::About_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e)
{
			MethodInvokerObjKeyEventArgs^ mi = gcnew MethodInvokerObjKeyEventArgs(bapsPresenterMain, &BAPSPresenterMain::BAPSPresenterMain_KeyDown);
			array<System::Object^>^ dd = gcnew array<System::Object^>(2) {bapsPresenterMain, e};
			this->Invoke(mi, dd);
}