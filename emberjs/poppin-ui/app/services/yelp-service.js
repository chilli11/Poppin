import Service, { inject as injectService } from '@ember/service';

export default class YelpService extends Service {
	@injectService apiService

	getLocMatch(locId) {
		return this.apiService.resources.getYelpMatch({ locId });
	}
}
