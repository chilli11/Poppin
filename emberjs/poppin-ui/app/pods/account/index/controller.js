import Controller from '@ember/controller';
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class AccountIndexController extends Controller {
	@service accountService;
	get isAuthorized() {
		return this.accountService.authInfo && this.accountService.authInfo.authorized;
	}

	@action
	refreshRoute() {
		this.transitionToRoute('account.me');
	}
}
