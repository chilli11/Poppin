import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AccountConfirmEmailRoute extends Route {
	@service accountService;

	queryParams = {
		t: { refreshModel: true }
	}
	
	model(params) {
		return this.accountService.confirmEmail(params.user_id, params.t);
	}
}
