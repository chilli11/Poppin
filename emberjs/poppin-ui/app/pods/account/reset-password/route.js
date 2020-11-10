import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AccountResetPasswordRoute extends Route {
	@service accountService;

	queryParams = {
		t: { refreshModel: true }
	}
	
	model(params) {
		return this.accountService.confirmResetToken(params.user_id, params.t)
			.then((response) => { 
				return { userId: params.user_id, token: response.token };
			});
	}
}
